# YM-HISTORY-NORM-1 — local/relative reconstruction audit (Iteration 6)

Scope: replace the impossible global-history norm by a local/relative connected-history/activity Banach reconstruction. No K_0 and no STEP_0.

## 1. GLOBAL-HISTORY H_0 target is too strong

Consider N independent blocks. On each block b let u_b in {0,1}. A raw history is h=(h_1,...,h_N) in {0,1}^N and

chi_h(u)=product_b 1_{u_b=h_b}.

Then

chi_h in {0,1},
sum_h chi_h(u)=1,
||chi_h||_infinity=1.

Therefore the naive absolute global-history majorant is

H_0^global(N)=sum_h ||chi_h||_infinity=2^N.

It diverges exponentially with volume.

Yet for product reference measure 2^{-N} sum_u and a source J_A supported on a fixed set A,

Z_N(J_A)/Z_N(0)
 = 2^{-|A|} sum_{u_A} exp[J_A O(u_A)],

which is independent of N.

Remote history variables sum out exactly because

sum_{h_b in {0,1}} 1_{u_b=h_b}=1

for every remote block.

Conclusion:

GLOBAL-HISTORY H_0 TARGET: TOO STRONG.

The failure is caused by taking individual sup norms before using partition/factorization. No future proof should require sum over norms of all complete global histories.

## 2. Source structure says to pass to connected history components

Direct-primary CMP119 gives, for a disjoint decomposition of the final large-field region

Z_k = X_1 disjoint-union ... disjoint-union X_n,

the exact factorization

T_k(Z_k)=product_i T_k(X_i).                              (CMP119 2.19)

For one component T_k(X), the operation factorizes into its successive-scale pieces (2.20).

Direct-primary CMP122-II goes further. After the local R-operation:

- the expression is decomposed into connected localization domains X'_p;
- for fixed connected components the source resums all expressions determining those components;
- Eq. (1.90) is a polymer expansion with product of connected activities F(X'_p);
- Eq. (1.97) proves exponential tree decay of F(X');
- Eq. (1.98) exponentiates the polymer expansion:
      {...}' = exp R'^(k) = exp sum_X R'^(k)(X);
- Eq. (1.99) gives exponential decay for the connected log-activities R'^(k)(X);
- Eq. (1.100) gives the sharper regular-sector bound
      |R'^(k)(X)| <= exp[-p_0(g_k)] exp[-kappa d_k(X)]
  for the second-group terms.

Thus a large portion of the global history combinatorics has already been converted by the source itself into CONNECTED LOCALIZATION ACTIVITIES. Independent remote components are not supposed to be counted as different states in the local norm.

Important limitation:
this is directly proved for the R-operation/local large-field-history sector. We have not proved that the entire compressed outer sum in CMP119 (2.18) is globally identical to one scalar hard-core polymer gas.

## 3. Correct target Banach space: connected relative log-density/activity space

Let P_k be the family of connected localization domains X in D_k and let A_X be the local analytic Banach space used for the source activity on X.

For mu>=0 define

||K||_{loc,mu;k}
 =
 sup_{Delta in pi_k}
 sum_{X in P_k: X contains Delta}
 exp(mu d_k(X)) ||K_X||_{A_X}.

Let K_{loc,mu;k} be the family of activities with finite norm.

This is a Banach space:
a Cauchy sequence is coordinatewise Cauchy in every A_X; coordinatewise completeness gives a limit; Fatou/liminf applied to every anchored sum gives the norm bound and convergence.

The extensive vacuum constant is NOT inserted as a total number E|Lambda|. Either:
- quotient log-densities by additive field-independent constants; or
- keep the vacuum ENERGY DENSITY e as a separate scalar coordinate.

Define the relative connected target

D_rel,k
 =
 C_vac direct-sum
 K^E_{loc,mu;k}
 direct-sum K^R_{loc,mu;k}
 direct-sum K^B_{loc,mu;k}
 direct-sum T_{loc,mu;k},

where T uses the operator norm for connected T-components.

This is the preferred target among the alternatives in the prompt.

Reason:
- vacuum/extensive factor is separated;
- locality is explicit in X;
- CMP119/CMP122 outputs are already local action/T activities;
- RG naturally acts on these objects;
- marked/source derivatives can later be placed in the same anchored norm;
- local source ratio W(J)=log Z(J)-log Z(0) becomes a derived normalized observable rather than the state-space norm itself.

Local marginals are not chosen as the primary coordinates because computing them requires another integration over the complement and obscures the local RG structure.

## 4. Direct source summability theorem from CMP109

CMP109 p.257 defines d_j(X) as shortest tree length divided by M and then states that after rescaling pi_j-cubes become unit cubes.

For sufficiently large lambda, the argument in Eq. (0.26) gives the source geometric estimate

C_geo(lambda)
 :=
 sup_{j,Delta in pi_j}
 sum_{X in D_j: X contains Delta}
 exp[-lambda d_j(X)]
 < infinity,

with C_geo(lambda)=O(1) independent of scale and total torus volume.

This source inequality is better for the present purpose than the very crude explicit DFS count from Iteration 5.

Consequently:

If
||K_X|| <= A exp[-lambda d_j(X)]

and 0<=mu<lambda is such that lambda-mu remains in the source summability regime, then

||K||_{loc,mu;j}
 <= A C_geo(lambda-mu).                                  (L1)

This is a genuine cutoff/volume-independent anchored Banach estimate.

## 5. Translation of CMP122-II history bounds to a local activity bound

### 5.1 Connected T-history component

CMP122-II (1.75) gives

||(T'_k(X)1)^(-1) T'_k(X)F||
 <= exp(3 sup|sigma|) ||F||.

Eq. (1.89) gives

T'_k(X)1
 <= exp[-a_0 p_0(g_k)],
a_0=2/(1+beta_0).

The paragraph immediately after (1.89) states that for an arbitrary large-field region one has the improved version with an additional source tree penalty

exp[-lambda_T d_k(X)].

Hence, for the scalar history-component weight,

tau_k(X)
 :=
 |T'_k(X)1|
 <=
 exp[-a_0 p_0(g_k)]
 exp[-lambda_T d_k(X)].                                  (L2)

For the normalized operator acting on F, multiply the right-hand side by exp(3 sup|sigma|).

Therefore, for any mu with lambda_T-mu in the CMP109 summability regime,

H_{0,T}^{loc}(mu)
 :=
 sup_Delta
 sum_{X contains Delta}
 exp(mu d_k(X)) tau_k(X)

 <=
 exp[-a_0 p_0(g_k)]
 C_geo(lambda_T-mu).                                      (L3)

If all effective couplings lie in the source interval (0,gamma], set

p_min(gamma)=inf_{0<g<=gamma} p_0(g).

Then

H_{0,T}^{loc}(mu)
 <=
 exp[-a_0 p_min(gamma)]
 C_geo(lambda_T-mu),                                      (L4)

uniformly in volume, UV cutoff and RG depth k.

No numerical value is invented.

### 5.2 Exponentiated connected R-history activities

CMP122-II (1.97) proves, after the history/integration sums used in (1.79)-(1.89),

|F(X')|
 <= c_1 exp[-(1+beta) kappa d_{k,Y}(X')],

where c_1 is either a source small parameter alpha^{1/3} or exp[-p_0(g_k)] depending on the component's relation to the large-field core.

The source then performs a convergent exponentiated cluster expansion and obtains (1.99):

|R'^(k)(X)|
 <= O(1)c_1
    exp[-(1+beta/2) kappa d_{k,Y}(X)].                    (L5)

For the regular second group, Eq. (1.100) sharpens this to

|R'^(k)(X)|
 <= exp[-p_0(g_k)] exp[-kappa d_k(X)].                   (L6)

Thus the source itself implements

global/raw history data
 -> connected history/localization polymer
 -> exponentially decaying log-activity.

For (L6), CMP109 (0.26) immediately gives

sup_Delta sum_{X contains Delta}
 exp(mu d_k(X)) |R'^(k)(X)|
 <=
 exp[-p_0(g_k)] C_geo(kappa-mu),                          (L7)

whenever kappa-mu is in the source summability regime.

For the boundary/large-field-connected activities in (L5), the metric is relative to the large-field core. The full source construction later places these terms in the B-sector, for which CMP119 supplies standard d_j(X) decay. A clean fixed anchored bound for the intermediate relative metric is therefore not asserted separately here.

## 6. Connected-history equivalence near A

After the connected resummation, define a connected history polymer gamma by:
- its connected localization support X;
- the internal raw/S/T history data already resummed into its activity/operator.

For a finite block region A, define the A-relevant subcollection

Gamma_A={gamma: supp(gamma) intersects A}

(or the source-dependency collar A^+ when a local activity depends on a collar).

Two post-resummation history configurations Gamma,Gamma' are A-equivalent,

Gamma ~_A Gamma',

iff they induce the same A-relevant connected activities.

For the local relative LOG-DENSITY projection this gives exact remote cancellation:

Pi_A K(Gamma)=Pi_A K(Gamma').

Remote disconnected components simply do not occur in Pi_A.

This is source-faithful because CMP119 factorizes T over disjoint large-field components and CMP122-II exponentiates the connected R-operation components.

Caveat:
This exact statement is for the local connected effective-action/T representation. A theorem asserting exact equality of fully integrated physical local marginals for arbitrary far-away changes is NOT proved and would generally be too strong. For physical source ratios, the expected statement is exact cancellation of truly disconnected source-independent clusters plus exponential suppression of connected bridging clusters.

## 7. GLOBAL H_0 replaced by anchored local H_0/H_1

For connected activity maps Phi_gamma(F), define

H_0^loc(R,mu)
 =
 sup_B sum_{gamma contains B}
 e^{mu size(gamma)}
 sup_{||F||<=R} ||Phi_gamma(F)||,

H_1^loc(R,mu)
 =
 sup_B sum_{gamma contains B}
 e^{mu size(gamma)}
 Lip_R(Phi_gamma)/w_gamma.

These are the correct replacements for sums over complete global histories.

The source estimates (L3),(L7) prove finite H_0^loc for important history-generated sectors.

A cutoff/RG-depth-uniform H_1^loc for the full Balaban source map is not yet extracted.

## 8. YM-LOCAL-ACTIVITY-VECTOR-1 — TYPED ACTIVITY VECTOR

Iteration 6 wrote the formal sum

Act_A(F)=sum_gamma Phi_gamma(F)

although Phi_gamma(F) may belong to different local spaces C_gamma. That formula is not intrinsically typed.

Iteration 7 replaces it by the direct-sum target

D_A^(mu)
=
l^1({C_gamma}_{supp(gamma) intersects A^+};e^{mu d(gamma)})

and canonical injections

iota_{gamma,A}:C_gamma->D_A^(mu).

The correct ACTIVITY-VECTOR definition is

Act_A(F)
=
sum_{supp(gamma) intersects A^+}
iota_{gamma,A} Phi_gamma(F).

Act_A is not a physical local density/marginal reconstruction. A future synthesis map

S_A:D_A^(mu)->F_A

must be handled separately; its boundedness is currently UNKNOWN.

If

h_0^(mu)
=
sup_B sum_{gamma contains B}
e^{mu d(gamma)}
sup_{||F||<=R}||Phi_gamma(F)||,

h_1^(mu)
=
sup_B sum_{gamma contains B}
e^{mu d(gamma)}
Lip_R(Phi_gamma),

then

||Act_A(F)||_{D_A^(mu)}
<=|A^+|h_0^(mu),

||Act_A(F)-Act_A(G)||_{D_A^(mu)}
<=|A^+|h_1^(mu)||F-G||.

Status:
PROVED. This supersedes the untyped Iteration-6 statement.

See history_c1.md and the corrected lemma in lemmas.md.

## 9. Stronger source/log-ratio version

Suppose a source J_A modifies connected log-activities K_X(J_A) and

Delta_A K_X := K_X(J_A)-K_X(0).

If strict source locality holds,

Delta_A K_X=0 whenever X cap A^+=empty,

then the relative local effective action satisfies

||sum_X Delta_A K_X||
 <= |A^+|
    sup_B sum_{X contains B} ||Delta_A K_X||.              (L8)

All remote connected components cancel exactly in the difference.

If instead

||Delta_A K_X||
 <= C||J_A|| exp[-nu dist(X,A)] exp[-lambda d(X)],

then the same anchored summation produces a volume-independent quasi-local bound and an exponentially small tail outside a radius r.

This topology can therefore formulate the desired future estimate for two marked sources A,B:
only connected activities capable of linking both marks contribute, so a source-derivative bound with an exponential tree weight would imply
|partial_A partial_B W| <= C exp[-mu dist(A,B)].

The actual Yang-Mills source derivative bound is NOT proved in this iteration.

## 10. Local source ratio and remote factor cancellation

For a convergent polymer/cluster representation of a normalized partition function, write

log Z_Lambda(J)
 =
 C_Lambda(J)+
 sum_{C connected} phi_J(C).

If J=J_A is local and phi_J(C)=phi_0(C) for C not connected to A, then

W_Lambda(J_A)
 :=
 log Z_Lambda(J_A)-log Z_Lambda(0)

 =
 [C_Lambda(J_A)-C_Lambda(0)]
 +
 sum_{C connected to A}
 [phi_J(C)-phi_0(C)].                                     (L9)

All completely remote connected clusters cancel term by term.

Under an anchored bound

sup_B sum_{C contains B}
 |phi_J(C)-phi_0(C)| <= q_A,

one gets

|W_Lambda(J_A)-Delta C_A|
 <= |A| q_A,

uniformly in Lambda.

This is the exact mathematical mechanism wanted from "remote-history cancellation".

Balaban's R-operation already supplies the connected localization architecture needed for such a representation, but a fully source-dependent implementation of (L9) for the Yang-Mills observables is still part of the separate source-control problem.

## 11. Why connected-log-density is the preferred target

Comparison of the requested variants:

A. local marginal rho_A:
volume-normalized but requires integrating the complement and obscures the RG-local activities. Not preferred as the primary state coordinate.

B. normalized local density:
same issue; useful downstream, not native to Balaban's effective-action representation.

C. source ratio Z(J)/Z(0):
excellent physical normalized object and remote vacuum cancellation is automatic, but it is an output of another global integration.

D. connected-log-density/polymer activity:
DIRECTLY produced by Balaban's exponentiated cluster expansions, removes extensive constants, exposes locality, and admits volume-independent anchored norms. CHOSEN PRIMARY TARGET.

E. source functional W(J):
chosen as the natural source extension/output of D, not as the base state space.

## 12. Tree normalization audit

Direct CMP109 p.257 says:
- in the lattice T_xi with xi=L^{-j}, pi_j cubes have side M in lattice units (physical side M xi);
- tree length is measured in the continuous space and divided by M;
- equivalently, the space is rescaled so pi_j cubes become unit cubes and d_j is measured in that scale;
- a shortest tree may be chosen along cube edges.

Therefore there is NO missing factor M in the dimensionless d_j used in the activity bounds.

A cube edge has physical length M xi, but after dividing by M xi / using the source rescaled coordinate it contributes one unit to d_j.

Thus the Iteration-5 coercivity estimate

|X| <= 2^d(d_j(X)+1)

and in d=4

|X| <= 16(d_j(X)+1)

has the correct normalization.

For Banach summability, however, the direct source Eq. (0.26) should be preferred over the very crude DFS constants.

## 13. What is solved and what is not

SOLVED / PROVED:
- global H_0 over complete histories is an unnecessarily strong and generally volume-divergent target;
- connected-log-density/activity space is the correct local/relative target;
- CMP109 gives volume-uniform anchored summability of exponential tree weights;
- CMP122-II provides source-derived local T-history and R-history connected activity bounds;
- the local activity-vector theorem YM-LOCAL-ACTIVITY-VECTOR-1;
- exact remote cancellation at the level of local connected log-activity projection;
- tree normalization has no missing M.

NOT YET SOLVED:
- a full theorem rewriting the ENTIRE compressed CMP119 Eq. (2.18) outer-history sum as one connected polymer gas in a single fixed Banach chart;
- cutoff/RG-depth-uniform Lipschitz bounds for the map from pre-resummed Balaban history data to the connected activity coordinates;
- fixed normalized analytic charts across historical scales;
- a fully source-dependent W(J_A) theorem for Yang-Mills observables.

STATUS:
BLOCKED AT YM-HISTORY-NORM-1, but the blocker has narrowed to the outer-history-to-connected-activity map / uniform Lipschitz chart. Global history summability itself is no longer the right problem.


## Iteration 9 correction — canonical versus native R' chart

The anchored Banach-space machinery itself is unchanged.

However, whenever an R' cluster/log submap is used, distinguish:

1. CANONICAL chart:
   explicitly defined carrier-overlap hard-core gas plus exact-union connected logarithm. Here the geometry is proved:
   C_inc=1, c_stitch=2, and D_eta H_gamma=0 unless eta subset gamma.

2. BALABAN NATIVE chart:
   CMP122-II (1.90) has a primed component-family sum, but the extracted primary passage has not supplied an iff characterization of that prime. Native C_inc and c_stitch are therefore not fixed.
   After (1.98), native R'^(k)(X) is defined through cited series (7.13) [I], whose target-localization formula has not been recovered.

Consequently the anchored canonical C^1 theorem cannot be transported to native R' coordinates without YM-RPRIME-NATIVE-COMPAT-1 and YM-RPRIME-NATIVE-COORD-1.


## Iteration 9 final — native R' chart restored with corrected geometry

The temporary canonical/native uncertainty is resolved for the regular R' cluster/log step.

Native incompatibility is cube-or-wall contact, not literal carrier overlap.

Use

Supp_*(eta)=N_wall[Car(eta)],
C_inc=9,
c_stitch=sqrt(7),

and therefore

r_*^native
=
(a/9)exp[-16a-sqrt(7)theta].

The legacy source citation for R'^(k)(X) is cross-version identified with published CMP116 Eq. (2.13), which groups connected clusters by exact target union. Hence the exact-union derivative support theorem applies to native R' coordinates.

The broader history Banach-space blockers outside this sector are unchanged.
