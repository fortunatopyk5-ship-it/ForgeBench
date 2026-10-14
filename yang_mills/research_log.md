# Research log

## 2026-09-25 — Iteration 1

Idea:
Reduce the gap half of the problem to a physical-scale Euclidean-time clustering estimate and investigate whether functional inequalities can supply it.

Mathematical setup:
Wilson lattice measure on compact link variables with Haar measure; avoid gauge fixing for the main reduction. Use reflection positivity to obtain transfer evolution. Separate the existence/UV problem from the IR gap problem.

Verified:
1. Spectral theorem proves YM-BRIDGE-1: a common exponential Euclidean-time decay rate on a dense centered local algebra excludes spectrum below that rate.
2. Correct lattice scaling target is gamma(a)>=m_* a. Requiring a lattice-unit exponent bounded below by a positive constant independent of a would be much stronger than needed.
3. Strong-coupling SU(N) result of Shen–R. Zhu–X. Zhu gives LSI/PI and exponential correlation decay only for |beta|<1/[16(d-1)]. In d=4: |beta|<1/48.
4. Their proof shows PI alone is not the spatial-mixing mechanism: PI gives decay in auxiliary semigroup time, while commutator/locality bounds control propagation; optimizing the two yields spatial decay.
5. This suggests a scale-invariant target lambda_a/v_a >= c a Lambda_YM.
6. Direct Bakry–Emery continuation fails because K_S becomes negative as beta grows.
7. Pointwise finite-beta gap, analytic continuation, no finite-beta phase transition, area law, or Lambda_YM>0 alone do not imply the required continuum spectral gap.

Problem encountered:
No weak-coupling, volume-uniform estimate with correct a(beta) scaling was obtained.

Result:
STATUS: PROMISING ROUTE — NOT A PROOF.

Next step:
Prove an abstract PI+finite-propagation => covariance theorem with explicit constants, then search for an RG mechanism that gives lambda_a/v_a at the asymptotically-free trajectory without assuming Dobrushin mixing as an input.


## 2026-09-25 — Iteration 1b

New theorem:
YM-SEMIGROUP-1. Under a Poincare variance decay and a weighted gradient-propagation bound for a local reversible diffusion, covariance decays in lattice distance with rate at least
kappa lambda/[2(lambda+v_kappa)].
This makes explicit the hidden second ingredient behind the Shen–Zhu–Zhu strong-coupling argument.

Adversarial finding:
A global auxiliary Markov spectral gap is not obviously a minimal target. Global/topological bottlenecks may make it small without forcing long-range local physical correlations.

New reduced route:
YM-RG-LANDING-1. Run exact RG from the asymptotically-free UV and prove that at a_* comparable to Lambda_YM^{-1} the effective polymer activities obey a uniform Kotecky-Preiss exponential norm bound. Standard cluster expansion would then yield an O(Lambda_YM) correlation rate. Reflection positivity can remain entirely at the original Wilson lattice level; it need not be preserved by each blocking transformation.

Current best next attack:
Make YM-RG-LANDING-1 less black-box by splitting it into:
(a) UV Balaban-controlled steps up to a matching scale;
(b) a finite crossover lemma for the remaining O(1) range of effective coupling;
(c) entry into a rigorous strong-mixing/polymer domain.
The likely genuinely new content is (b).


## 2026-09-25 — Iteration 1c

Reduction sharpened:
Introduce YM-CROSSOVER-1. After the UV RG reaches a fixed matching coupling g_match, prove that a finite, cutoff-independent number M_G of exact RG steps sends the full closed/bounded matching tube of effective interactions into a Kotecky-Preiss strong-mixing domain.

Reason this matters:
The number of UV steps diverges as a->0, but those are the asymptotically-free/small-coupling steps for which constructive RG is the natural tool. The genuinely nonperturbative crossover starts from a fixed g_match, so its required scale interval is fixed; if it can be controlled, it is a finite-step problem.

Adversarial warning:
A scalar running coupling is not enough. The inclusion must be in a norm controlling the entire generated effective action, including irrelevant operators and large-field/polymer terms.


## 2026-09-25 — Iteration 2: YM-CROSSOVER-1 only

Adversarial result 1:
The previous reduction was too optimistic if it tracked only a running coupling. Explicit toy RG maps show that a coupling can run monotonically to a nominal strong-coupling region while generated irrelevant/polymer coordinates grow or settle outside every small-activity domain.

Adversarial result 2:
Measure-only landing in a KP domain does not by itself control original microscopic correlations. The exact conditional-covariance decomposition exposes the missing term. A hidden-sector toy model gives a direct counterexample. Added YM-RG-SOURCE-1.

Concrete state space:
Defined B_{alpha,mu,p,rho}, a weighted Banach space of globally defined gauge-invariant interactions on connected 4D block polymers. Local norms are explicit C^p norms built from invariant Lie derivatives and supremized over collar/boundary link fields. See crossover_spec.md.

Concrete landing domain:
Defined exact scalar super-polymer activities by expanding the global interaction, grouping connected overlapping families and integrating against product Haar. Proved that
Q_{alpha,mu}<alpha
implies the Kotecky-Preiss condition and uniform cluster expansion.

Finite-step reduction:
Proved YM-TUBE-1. YM-CROSSOVER measure inclusion follows from a finite list of one-step interval enclosures plus a final KP inequality.
Proved YM-RG-DRIFT-1. A uniform drift delta>0 together with an invariant remainder tube implies a cutoff-independent finite number of crossover steps; finite M is therefore no longer assumed.

Matching correction:
Balaban's verified published claims provide a substantial UV RG package, but do not automatically give the specific closed/bounded K_match in the new global norm. Added YM-MATCH-EXTRACT-1 as a separate unresolved input.

Continuum bridge audit:
Strengthened the cutoff bridge to YM-CUTOFF-BRIDGE-2. Once each fixed-cutoff theory has a genuine transfer gap m_a, the semigroup inequality
C_a(t+s)<=e^{-m_a s}C_a(t)
passes to the continuum using positive-time correlator convergence; no equal-time norm convergence or uniform cluster prefactor is required.

Validated numerics:
Added scripts/verify_crossover_tube.py, an exact Fraction-based interval checker, and a toy certificate. It certifies only the finite induction conditional on genuine RG enclosure formulae.

Result:
STATUS: YM-CROSSOVER-1 REDUCED TO LEMMAS A/B/C — NOT PROVED.

A = YM-MATCH-EXTRACT-1.
B = Yang-Mills one-step invariant-tube/drift enclosures ending in Q_KP<alpha.
C = YM-RG-SOURCE-1.

Next action:
Do not open a new mass-gap route. Extract an actual one-step Balaban gauge-RG map/bounds into the variables beta,r of crossover_spec.md and determine whether the first nontrivial tube step can be proved analytically.


### Iteration 2 final refinement

Proved YM-RG-DERIV-1:
the first and second Frechet derivatives of exact block integration are conditional expectations and minus conditional covariances.

Proved YM-RG-TAYLOR-1:
a full infinite-dimensional one-step tube inclusion is implied by five scalar families of rigorous bounds (center flow, generated remainder, relevant/irrelevant mixing, irrelevant amplification, second derivative). This is the closest result of this iteration to the target
YM-CROSSOVER-1 <=> finite explicit list of inequalities.

Exact continuation point:
extract the first matching interval I_0 and radius R_0 from Balaban-type bounds, then prove numerical/analytic enclosures f_0^±,H_0,A_0,B_0,C_0. Do not move to another mass-gap route before this first tube step is either certified or shown impossible in the chosen norm.


## 2026-09-25 — Iteration 3: YM-MATCH-EXTRACT-1 + STEP_0 validity only

Compactness audit:
The earlier description of K_match as compact in an infinite-dimensional Banach space was incorrect. The matching object is now only closed/bounded. All tube results use universal inequalities/suprema and do not need attainment.

Source/topology audit:
Equation-level source extraction shows that Balaban's effective density is domain-history dependent and includes characteristic/T-operation structure. The E/R/B pieces are localized and analytic on restricted regularity domains; large-field terms are controlled separately. This does not furnish a global all-field C^p remainder bound.

Verdict:
The previous global B_{alpha,mu,p,rho} remains a legitimate abstract Banach space but is not currently justified as the matching topology. YM-MATCH-EXTRACT-1 was reformulated in a domain-indexed native activity topology.

Concrete source shapes recorded:
- CMP119 (2.18): domain-history density expansion;
- CMP119 (2.23): classical + E/R/B action decomposition;
- CMP119 (2.24): localized 1/g^2 coupling recursion;
- CMP119 (2.31): R-term pointwise exponential decay;
- CMP119 (2.42): B-term pointwise exponential decay;
- CMP122-I (1.70): large-field C-term exponential bound;
- CMP122-II (1.99)-(1.100): post-R localized remainder bounds;
- CMP122-II Theorem 1: preservation of the CMP119 inductive density class while effective couplings remain sufficiently small.

Proved:
- YM-COMPACTNESS-CORRECTION-1.
- YM-RG-CHAINRULE-1 (abstract full-R chain rule).
- YM-NATIVE-TAIL-1 (pointwise polymer decay + rooted counting => anchored norm).

Full-R audit:
E derivative identities are valid at finite volume, but E/S/L Banach mapping and localization boundedness remain open; therefore C_0 is not instantiated.

STEP_0:
Not attacked with constants because no mathematically legitimate K_0 containing the actual matching output has been constructed.
All f_0^±,H_0,A_0,B_0,C_0 remain UNKNOWN.

Result:
STATUS: BLOCKED AT YM-MATCH-EXTRACT-1.

Exact continuation point:
Build the exact native density-space predicate and rooted summability constants from CMP119/CMP122, then establish a closed/bounded K_0 at fixed small g_match. Only then attempt the first full-R STEP_0 certificate.


## 2026-09-26 — Iteration 4: mathematical validity of Balaban-native matching space

Scope:
Only YM-MATCH-EXTRACT-1 state-space validity. STEP_0 was prohibited and not attempted.

Strongest correction:
The native weighted l-infinity H-infinity coefficient product is not a state space of actual effective densities. The correct mathematical object is a nonlinear reconstruction graph over a closed linear compatibility layer.

History audit:
The full density sums over admissible domain histories. A sup over histories does not control that sum. No exact-disjointness theorem, cutoff-uniform finite-overlap constant, or complete source history weight was verified. Added YM-HISTORY-SUP-FAIL-1 and YM-HISTORY-WEIGHT-1.

Metric audit:
The source ledger identifies CMP109 d_j(X) as a shortest-tree localization size divided by M. Diameter-only exponential counting is false. For a bounded-degree block graph, connected m-block animals through an anchor are bounded by Delta^{2(m-1)} using a canonical DFS traversal. Hence d-shell counting is reduced to the Balaban-specific coercivity |X|<=a_0+a_1 d_j(X). History entropy remains separate.

Analytic-domain audit:
A raw common analytic core may collapse because historical analytic radii depend on g_j and early UV couplings approach zero. A normalized source-dependent analytic chart is required; no uniform chart/collar theorem was extracted.

Coupling audit:
The native coordinate is a localized c_j(x)=1/g_j^2(x). A scalar c alone is insufficient near domain boundaries. The augmented state must include a coupling-profile defect q.

Primary-source audit:
CMP119/CMP122 direct equation-level verification could not be upgraded: repository DOI links redirect to Springer endpoints inaccessible to the available browser. Equation-level formulas therefore retain secondary visual-transcription status. Paper-level metadata/abstracts were verified. The CMP109 d_j definition remains source-extracted through the audit ledger.

Result:
STATUS: BLOCKED BEFORE K_0.

Exact continuation:
Extract exact primary admissible-history/nesting/characteristic structure and history summation mechanism; prove Balaban-specific tree coercivity; construct normalized analytic charts with a positive fixed reference collar; then prove reconstruction continuity and native RG preservation.


## 2026-10-13 — Iteration 5: exact Balaban history and history-sum mechanism

Scope:
Only YM-HISTORY-RECON-1. STEP_0 and K_0 construction were not attempted.

Primary-source breakthrough:
Direct article PDFs for CMP119 and CMP122-II became accessible and were inspected page-by-page. History-related claims previously carried as secondary transcriptions were upgraded to direct primary verification.

History structure:
CMP119 Eq. (2.1) gives nested/interleaved Omega/Lambda localization domains. S_j lies in Omega_j intersect Lambda_j^c with source cube/collar conditions. Eq. (2.18) sums only the outer Omega/Lambda histories; the S_j sum is explicitly inside T_k.

Raw versus compressed history:
CMP119 Eqs. (3.2),(3.3),(3.16) are exact hard {0,1} partitions of unity over raw P/Q/R sets. These raw choices are not retained as final history coordinates: fixed P,Q choices producing the same Omega are resummed into zeta(Omega^c), and S is summed in T. Therefore chi_k(Omega_k) in Eq. (2.17) is not a complete-history characteristic and sum_H chi_H=1 must not be asserted at the compressed level.

T-operation:
Directly verified as an integration/operator composition with characteristic factors, RG delta constraints, gauge fixing and internal S sums; it factorizes over disjoint large-field components and into ordered one-step operations.

History entropy:
CMP122-II directly states that summations over admissible sequences are replaced by exp{O(1)(MR_j)^(-d)|Z_j|} factors. Large-field characteristic functions yield negative p_0(g_j)^2/tree-size and p_0(g_j) penalties. Eq. (1.79) combines the positive entropy and negative energy terms; the induction (1.80)-(1.88) pays the cost across scales. Eq. (1.89) yields a clean local T'_k(X)1 suppression. Thus the source does pay history entropy.

Operator majorant:
CMP122-II (1.75) gives the normalized local T'_k operator estimate. Combining (1.75) with (1.89) yields a source-local tau-type bound.

Coupling profile:
CMP119 (2.24) shows phi_j is a localization cutoff attached to Lambda_j and is not summed over. Once a deterministic Lambda->phi_Lambda convention is fixed, the localized profile is derived; q is removed as an independent physical coordinate.

New abstract theorem:
YM-HISTORY-RECON-ABSTRACT-1 proves absolute and Lipschitz reconstruction from fixed kernel weights H_0=sum tau_H and H_1=sum tau_H/w_H.

Remaining blocker:
Balaban's multiscale source estimate has not yet been transported into a single volume-independent local/relative density Banach target D with fixed history weights and fixed normalized analytic charts. A plain global sup norm of the unnormalized density is structurally wrong because the density is volume-extensive.

Result:
STATUS: BLOCKED AT YM-HISTORY-RECON-1.

Exact continuation:
Define D, extract fixed tau_H/w_H from (1.79)-(1.89), and prove H_0,H_1 uniform. Then establish an open ambient B_amb on which the native RG extends smoothly.


Iteration 5 tree-side-task closure:
Direct CMP109 printed p.257 was inspected. The source defines d_j(X) by a shortest tree graph along localization-cube edges. In d=4 every tree vertex belongs to at most 16 unit cubes, so |X|<=16(d_j(X)+1). Since the face-adjacency graph has degree 8, canonical DFS gives at most 64^{m-1} connected m-cube sets through an anchor. Therefore
# {X contains B: n<=d_j(X)<n+1}
< (64^32/63) exp((96 log 2)n).
This is crude but explicit, scale/cutoff/volume independent, and closes fixed-history polymer counting. It does not address history multiplicity; that is paid separately by CMP122-II's admissible-sequence entropy/large-field-energy mechanism.


## 2026-10-13 — Iteration 6: local/relative history norm

Scope:
Only YM-HISTORY-NORM-1. No K_0 and no STEP_0.

Adversarial result:
The previous global complete-history target H_0=sum_H tau_H is too strong. A product toy model with N independent binary hard-partition choices has 2^N complete histories with unit sup norm, while a fixed local source ratio is independent of N. The divergence is an artifact of summing global history norms before remote partition cancellation/factorization.

Target topology:
Selected the connected relative log-density/activity space with anchored norm
sup_Delta sum_{X contains Delta} exp(mu d_k(X)) ||K_X||.
Vacuum energy density is separated; no uniform norm of the raw extensive unnormalized density is requested.

Primary-source bridge:
CMP109 (0.26) directly provides volume/scale-uniform anchored summability of exp[-lambda d_j(X)] for sufficiently large lambda. Hence a pointwise source activity bound A exp[-Lambda d_j(X)] gives
||K||_{loc,mu} <= A C_geo(Lambda-mu).

CMP122-II translation:
- normalized local T' operator: (1.75);
- scalar large-field suppression: (1.89), with the source's arbitrary-region improvement adding tree-distance decay;
- connected R/history polymerization: (1.90)-(1.97);
- exponentiation to connected log-activity: (1.98);
- connected activity decay: (1.99);
- regular-sector bound: (1.100).

This yields explicit symbolic local H_0 bounds
H_{0,T}^{loc}(mu)
 <= exp[-a_T p_0(g_k)] C_geo(lambda_T-mu),
and
H_{0,R'}^{loc}(mu)
 <= exp[-p_0(g_k)] C_geo(kappa-mu),
under the source decay-reserve/small-coupling conditions.

Remote history:
At the connected log-activity level, disconnected remote components cancel exactly from the local projection. For a source ratio W(J_A), an abstract connected-cluster theorem gives termwise cancellation of source-independent remote clusters. Full Yang-Mills marked-source instantiation remains open.

New theorem:
YM-HISTORY-RECON-LOCAL-1:
anchored connected activity bounds h_0,h_1 imply
||Rec_A(F)||<=|A|h_0
and
||Rec_A(F)-Rec_A(G)||<=|A|h_1||F-G||,
uniformly in total volume.

Tree audit:
Direct CMP109 normalization confirms no missing M in dimensionless d_j. Source Eq. (0.26), not the crude DFS estimate, is the preferred summability input.

Result:
STATUS: BLOCKED AT YM-HISTORY-NORM-1.

Exact continuation:
prove a uniform local Lipschitz/analytic bound for the complete compressed outer-history/T -> connected-activity map in fixed normalized analytic charts. Global history H_0 should not be revived.


## 2026-10-13 — Iteration 7: bounded C^1 history resummation

Scope:
Only C^1 validity of history-resummation between connected anchored Banach spaces. No K_0 and no STEP_0.

Typing correction:
The old local reconstruction sum was ill-typed when activities lived in different local Banach spaces. Replaced it by a weighted l^1 direct-sum target D_A^(mu) with canonical coordinate injections. The corrected theorem gives |A^+|h_0 and |A^+|h_1 bounds.

Input/output spaces:
Defined explicit anchored B_in(mu_in),B_out(mu_out) with possibly different decay exponents. Positive loss mu_in-mu_out is allowed.

Jacobian:
Proved YM-HISTORY-JACOBIAN-1, a selector-based weighted Schur theorem with the correct orientation for anchored norms. Proved the quasi-local corollary: an off-diagonal e^{-nu dist(a,b)} kernel bound gives a uniform operator norm.

Cauchy route:
Proved Banach-valued Cauchy derivative theorem. Combined analyticity on a larger activity ball, pointwise output tree decay, and a support/collar rule into YM-CLUSTER-JACOBIAN-SCHUR-1. The input coordinate weight from Cauchy cancels exactly against the input Schur weight.

R' sector:
CMP122-II source decay changes from (1+beta)kappa before exponentiation to (1+beta/2)kappa after exponentiation; source-visible loss is beta*kappa/2. The cluster/log C^1 theorem is complete abstractly once a strict anchored input convergence radius and exact support/collar dictionary are supplied. Those two source translations remain open.

T sector:
For fixed local T component, exact differentiation of the normalized operator gives a covariance-type formula and an explicit bound using CMP122-II (1.75). Combined with tree suppression and local restriction, this yields an anchored derivative bound conditionally on the exact collar/source chart. Full T-history resummation C^1 is not proved.

Analytic charts:
A naive common raw field domain remains unjustified. A graded Banach product of scale-dependent H^infinity coordinate spaces is enough for bare activity-coefficient C^1, so shrinking raw radii are not by themselves fatal. But source/field derivatives and future homogeneous RG coordinates still require a cutoff-independent normalized analytic collar.

Source marks:
A local J_A is a legitimate tangent vector abstractly. Quasi-local first-mark propagation follows once the full Jacobian Schur estimate is source-instantiated.

Result:
STATUS: BLOCKED AT YM-HISTORY-NORM-1.

Exact continuation:
extract the strict anchored cluster convergence radius and exact support/collar relation for the R' cluster map first; this is now the cleanest candidate for a fully source-instantiated C^1 sector.


## 2026-10-14 — Iteration 8: regular R' cluster/log coefficient C^1

Scope:
Only the regular/second-group CMP122-II R' cluster/log coefficient map. No full outer-history map, no K_0, no STEP_0, no physical source differentiation.

Naming correction:
The typed local direct-sum object is now Act_A(F), not Rec_A(F). It is only an activity vector in D_A^(mu). A future physical synthesis map S_A:D_A^(mu)->F_A is separate and unproved.

Source tangent correction:
A physical source J_A is not automatically an activity tangent. Introduced a source space J_A and insertion map jmath_A:J_A->B_in; boundedness remains unknown.

Source object:
CMP122-II (1.90)-(1.97) gives the pre-exponentiation hard-core connected-component polymer gas and local activity F(X'). In the regular lane X' disjoint from Ycal, the relative metric equals d_k and
|F(X')|<=e^{-p_0(g_k)}e^{-(1+beta)kappa d_k(X')}.

Input norm:
||F||_in=sup_B sum_{eta contains B} e^{mu_in d_k(eta)}||F_eta||,
mu_in=(1+beta)kappa-lambda_in>0.
Thus
||F^src||_in<=C_geo(lambda_in)e^{-p_0(g_k)}.

Explicit convergence radius:
Using |eta|<=16(d(eta)+1), generalized KP weights
a_KP=a|eta|,
g_theta=theta d(eta)+2theta,
and 16a+theta<=mu_in, obtained the explicit sufficient radius
r_*=a e^{-16a-2theta}.
This is our theorem, not a guessed Balaban threshold.

Strict source radius:
Primary CMP119 p_0(g)=A_0(log g^{-2})^{p_*}->infinity. Shrink the fixed source small-coupling threshold gamma_R' so
r=C_geo(lambda_in)e^{-p_0(gamma_R')}<r_*,
then rho=(r+r_*)/2 gives a uniform Cauchy margin.

Support:
The canonical connected logarithm is grouped by exact union gamma=union_i eta_i. Therefore
D_eta H_R',gamma=0 unless eta subset gamma.
Selector s(gamma,eta) can be any deterministic cube in eta, so the coefficient-map collar is r_col=0.

Uniform holomorphy:
The generalized rooted KP estimate plus the tree-stitching lemma gives normal convergence in the output anchored tree norm on every closed rho<r_* input ball, uniformly in finite volume and scale. Banach-valued Weierstrass therefore gives uniform holomorphy.

Bounded C^1:
Banach Cauchy yields
sup_{||F||<=r}||D H_R'(F)||_{B_in(mu_in)->B_out(theta)}
<=q(rho)/(rho-r),
q(rho)=e^{16a+2theta}rho.
This closes the requested regular R' coefficient C^1 theorem under the explicit input decay-budget condition mu_in>0.

Quasi-local Jacobian:
A stronger Schur kernel bound follows if there are mu_out,nu>0 such that
lambda_res=theta-mu_out-nu
is in the CMP109 summability regime. No numerical source threshold guaranteeing this extra reserve is invented.

Result:
REGULAR R' BOUNDED C^1 PROVED.
QUASI-LOCAL SCHUR JACOBIAN PROVED UNDER AN EXPLICIT ADDITIONAL DECAY-BUDGET CONDITION.
Broader YM-HISTORY-NORM-1 remains open upstream and in the boundary lane.


## 2026-10-14 — Iteration 9: audit of (1.90) incompatibility and R' coordinates

Scope obeyed:
No upstream Eq. (1.91), no boundary B' estimates, no source marks, no K_0, no STEP_0.

A. PRIME AUDIT
Primary CMP122-II pp.387-388:
- raw data determine X'_0;
- Balaban defines a special component decomposition of X'_0;
- X'_0=union_p X'_p;
- the expression factorizes in those components;
- resummation at fixed components gives the primed polymer expansion (1.90).

Verified consequence:
members of each actual primed family have pairwise disjoint pi_k-cube carriers.

Not verified:
the converse that every pairwise carrier-disjoint family is allowed by the prime.
Therefore native incompatibility may be stronger than literal carrier overlap.

Native C_inc: UNKNOWN.
Native c_stitch: UNKNOWN.

Canonical carrier-overlap model:
C_inc=1,
c_stitch=2.

General corrected radius:
r_*^corr=(a/C_inc)exp[-16a-c_stitch theta].

B. COORDINATE AUDIT
Primary CMP122-II p.390:
exp R'^(k)=exp sum_X R'^(k)(X),
and native R'^(k)(X) is stated to be given by convergent series (7.13) [I].

Reference [I] is CMP109 (1987). The accessible published CMP109 has sections 0-5 and no searchable Eq. (7.13). The coordinate formula has therefore not been recovered.

Logical correction:
equality of the total logarithm does not imply equality of localization coordinates.

Verdict:
R' COORDINATE IDENTIFICATION = UNKNOWN (CASE 3).

The Iteration-8 native theorem claim is withdrawn and renamed:
YM-RPRIME-CANONICAL-C1-1.

What survives:
the canonical carrier-overlap exact-union log map is uniformly holomorphic/C^1 with the old explicit radius.

What does not survive:
the assertion that Balaban's native R'^(k)(X) coefficient map has already been proved C^1 in our coefficient topology.

New exact blockers:
YM-RPRIME-NATIVE-COMPAT-1;
YM-RPRIME-NATIVE-COORD-1.


### Iteration 9 final correction after deeper primary-source crosswalk

The temporary Case-3/UNKNOWN verdict was superseded.

A second primary-source pass found the exact published cluster formulas in CMP116:
- Eq. (2.11): zeta(Z,Z')=0 exactly for cube-or-wall contact;
- Eq. (2.12): connected Ursell logarithm;
- Eq. (2.13): localized coefficient grouped by exact union.

Why this applies to CMP122-II:
CMP122-II cites its first Mayer operation as "(7.1) [I]" and later native R'^(k)(X) as "(7.13) [I]". Its [I] bibliography entry is CMP109. The published CMP109 ends before this cluster localization and says it is deferred to the next paper. Published CMP116 is that next paper and its Sect. 2 begins with Mayer Eq. (2.1). This supplies a two-anchor legacy-numbering crosswalk:
7.1 -> 2.1,
7.13 -> 2.13.

Therefore:
COORDINATE IDENTIFICATION = EXACT (source-crosswalk verified).

Native compatibility is cube-or-wall, so the previous Iteration-8 constants were corrected:
C_inc: 1 -> 9,
c_stitch: 2 -> sqrt(7),
r_*: a e^{-16a-2theta}
   -> (a/9)e^{-16a-sqrt(7)theta}.

The native regular R' bounded C1 theorem survives after this stricter audit with the corrected radius.

No upstream Eq. (1.91) analysis was performed.


## 2026-10-14 — Iteration 10: upstream CMP122-II Eq. (1.91)

Scope obeyed:
No re-proof of F->R', no boundary B', no source marks, no K_0, no STEP_0.

Direct-primary p.388 transcription:
F(X') is the q / selected-T-domains / D sum of
[product_D int_0^1 dt(Y)]
[product_h T'_k(X_{j_h})]
[product_D V(Y)]
exp[sum_D t(Y)V(Y)],
with the primed index fiber consisting exactly of frozen localization data determining the component X'.

Primary p.389:
for X' disjoint from union_iY_i the q-sum begins at q=1; if X' contains a Y_i it may begin at q=0.

Functional-analytic result:
Eq. (1.91) itself is not where E/R/B extraction occurs. It is an analytic resummation map in two already-localized coordinates:
(T',V)->F.

A frozen-history Banach fiber was defined. Hard characteristics/history labels are discrete base data, not tangent variables.

A mixed norm was necessary:
- weighted sup/source regulator to preserve the source pointwise majorants;
- anchored l1 term to retain volume-independent locality.

Because the proof of (1.92)-(1.97) is an absolute-value majorant proof and its smallness inequalities can be imposed with strict slack, a uniform open coefficient radius R_src>1 exists after shrinking the fixed source coupling threshold.

Therefore the regular projected Eq. (1.91) map is Banach-holomorphic/C^1, with

||D M_1.91^reg||
<=
rho_F/(R_src-1).

Exact support:
D_eta F(X')=0 unless eta subset X'.

Adversarial correction:
this exact support is not yet enough for a quantitative upstream off-diagonal Schur bound in the mixed norm; a marked two-anchor majorant is still missing.

Raw-map blocker:
the first unproved submap is the earlier producer
(E,R,B,old-history local terms)->V(Y)
described on pp.376-377. Its output bounds (1.68),(1.69) are known, but a bounded coefficient-space difference/Frechet theorem has not yet been extracted.

Composition:
for reduced coordinates,
||D(R'_Bal o M_1.91^reg)||
<=
C_R' C_1.91.

Full raw composition remains conditional on a producer constant C_P.


### Iteration 10 final correction — source-native chart closes raw upstream C1

The intermediate verdict "blocked at P_V / E relevant extraction" was too low-level for the current inductive state.

CMP119 explicitly defines the current E_k as fully renormalized. Therefore the natural coefficient input to the current R-operation is E_ren, not E_raw. Likewise R is already vacuum-renormalized and B is already localized.

With these source-native coordinates:
- pp.376-377 V-production is a signed linear incidence resummation;
- the primary absolute summation gives a uniform row-majorant;
- after source-majorant normalization, ||P_V||<=1;
- P_Sigma:(T',E_ren,R_ren,B_loc,K_aux)->(T',V) is bounded linear;
- the already-proved holomorphic Eq. (1.91) map composes with P_Sigma.

Final source-native result:

YM-RPRIME-UPSTREAM-C1-1 = PROVED.

For the regular source-native chain,

||D U_Sigma||<=C_1.91,

and with downstream R',

||D(R'_Bal o U_Sigma)||<=C_R' C_1.91.

Exact raw support is expressed through the frozen incidence relation Out_Sigma(lambda):
a non-T input lambda can affect F(X') only if some Y in Out_Sigma(lambda) satisfies Y subset X'.

Still open:
the marked two-anchor incidence estimate needed for a quantitative upstream Schur kernel.

Separate lower-level issue:
E_raw -> E_ren, including relevant extraction (3.64), remains open but is not part of the current inductive-state upstream theorem.


### Iteration 10 adversarial revalidation

A fresh primary-PDF check was performed on CMP122-II printed pp.376-380 and 387-389.

Confirmed:
- Eq. (1.91) is exactly a frozen-index analytic map in the already-localized variables (T',V).
- For regular X' disjoint from the current large-field domains, the q-sum starts at q=1.
- The V-construction on pp.376-377 is a succession of resummations/sums once current renormalized local atoms are supplied.
- Whole-component V terms are diverted into T'_k and do not enter the regular V exponential.
- The T'_k normalization and normalized-operator bounds used in the local calculus are source-supported by (1.74)-(1.75).

Functional-analytic check:
The potentially weak step "source row bound => bounded producer" was made explicit as YM-V-PRODUCER-MIXED-NORM-1. Because the input norm contains a pointwise source-majorant regulator, the row estimate

sum_lambda |A(Y,lambda)|M_lambda <= C_res M_V(Y)

does imply a true Banach operator bound P_V:B_up->V, and the anchored output term follows by summing M_V(Y).

Thus the final source-native bounded-C1 theorem is unchanged.

Still open:
the row bound does not imply a marked two-anchor/off-diagonal Schur estimate for A(Y,lambda). That stronger locality theorem remains the first unresolved refinement.


## 2026-10-14 — adversarial upstream composition correction

Audited base cb0193b3bfe91c3c309b9431650d4785e15cd0a8 and the six requested current documents.

WITHDRAWN: unconditional source-instantiated upstream/reduced/composed bounded C1 claims of Iteration 10. The physical source majorant was promoted to an independent complex coefficient-ball bound without proving all operator/domain/geometric hypotheses survive perturbations. The producer row theorem is correct abstractly; its claimed source instantiation lacked the exact kernel and uniformity dictionary.

Preserved: exact recorded (1.91), frozen-label convention, finite-term algebra, accepted downstream native radius and theorem, and source-generated F anchored summability with the identical downstream exponent.

New explicit certificates: YM-191-COMPLEX-MAJORANT-1 and YM-UPSTREAM-IMAGE-RESERVE-1, plus the V operator row source instantiation. Source uniformity of physical inputs does not verify uniformity over arbitrary coefficient families and histories.

Verdict: UPSTREAM C1 GAP; COMPLEX BALL GAP; WHOLE-BALL IMAGE NO (not proved); COMPOSED C1 GAP; Sigma UNIFORMITY UNKNOWN. Conditional chain-rule domains/constants are now stated without applying them prematurely.

No downstream proof was redone. No K0, STEP0, boundary B' or other RG-sector work.


## 2026-10-14 — YM-191-COMPLEX-MAJORANT-1 continuation, V=0 first

Base: 33605c14555735826ad4d120c9b07db0a79c7886. Read all seven requested current files.

New proved results:
- sharp old-chart V=0 envelope criterion C_Tgeo(lambda_in-beta kappa)<infinity;
- explicit local evaluation-kernel counterexample, old mixed norm<=1/2 and divergent target sum for mu_F>=kappa on all connected polymers;
- stable conditional total-variation domination preserving hidden histories/integrations;
- genuinely open relative-density Banach balls with uniform local radius and normalization stability;
- conditional size/coupling-to-strong-decay embedding inequality.

Qualifications:
The old unqualified all-connected theorem is INVALID. The exact native restricted family and passing extra-reserve regimes are distinguished. The counterexample is not a physical Balaban history.

Stronger independent ordinary-decay weights pass V=0, but physical source embedding is unknown. Relative-kernel charts include source centers and are locally open, but do not supply missing decay/entropy. The full labelled positive majorant is specified with kernel/index data retained; its native uniform anchored bound is not established.

Concrete next blocker: q=1,D={Y}, source relative V geometry attached to first-class T cores, plus the exact native support/size and stable-domain dictionary.

Reduced C1 GAP; global Sigma uniformity UNKNOWN; strict image NOT PROVED. V-producer DEFERRED according to the reduced-first gate. Accepted downstream file unchanged; no forbidden-sector work.
