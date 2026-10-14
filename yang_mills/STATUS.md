# STATUS

Current objective:
Iteration 9 audits ONLY two already-claimed ingredients of the regular R' closure:
(A) the exact incompatibility encoded by the prime in CMP122-II (1.90);
(B) the coordinate identification between the canonical exact-union logarithm and Balaban's native R'^(k)(X).
No upstream Eq. (1.91), boundary B' analysis, K_0, or STEP_0 is attacked.

Current strongest result after audit:
The CANONICAL carrier-overlap exact-union cluster/log map remains uniformly holomorphic/C^1 with explicit radius

r_*^can = a exp[-16a-2theta],

because in that explicitly defined model

C_inc^can=1,
c_stitch^can=2.

However the previous source-instantiated claim about BALABAN NATIVE R' coordinates was too strong and is withdrawn.

Primary-source audit:
- CMP122-II pp.387-388 defines X'_0, a special component relation, writes X'_0=union_p X'_p, proves factorization in those components, and obtains the primed polymer expansion (1.90).
- Therefore every actual primed family has pairwise disjoint pi_k-cube carriers.
- The accessible primary passage does NOT explicitly establish the converse that every pairwise carrier-disjoint family is admissible under the prime. Thus native incompatibility is not proved to be iff carrier overlap.
- CMP122-II p.390 Eq. (1.98) writes exp R'^(k)=exp sum_X R'^(k)(X), but the text defines each native R'^(k)(X) through convergent series (7.13) [I].
- Reference [I] is CMP109. The cited formula (7.13) has not been recovered under that numbering in the accessible published CMP109 version. Equality of total logarithms therefore does NOT justify coordinatewise R'_Bal(X)=H_X^can.

Current theorem names/status:
- YM-RPRIME-CANONICAL-C1-1: PROVED for the explicitly defined carrier-overlap exact-union chart, under the stated input decay budget.
- YM-RPRIME-NATIVE-COMPAT-1: OPEN.
- YM-RPRIME-NATIVE-COORD-1: OPEN.
- Native Balaban regular R' coefficient C^1: NOT PROVED after this audit.

Corrected general KP geometry:
If a future native support theorem gives

#Supp_*(eta) <= C_inc |eta|

and every incompatibility edge can be stitched with cost c_stitch, then with

g_theta(eta)=theta d(eta)+c_stitch theta

the sufficient radius is

r_*^corr
=
(a/C_inc) exp[-16a-c_stitch theta].

For the canonical carrier-overlap chart only:

C_inc=1,
c_stitch=2,

hence the old radius is recovered.

Quasi-local Schur status:
Canonical chart: still PROVED under the explicit extra reserve

C_geo(theta-mu_out-nu)<infinity.

Native Balaban coordinates: additionally conditional on resolving YM-RPRIME-NATIVE-COMPAT-1 and YM-RPRIME-NATIVE-COORD-1.

Verified:
- CMP109: localization domains are unions of wall-connected pi_j cubes; d_j(X) is the shortest continuous-space tree meeting all cubes after unit-cube rescaling.
- Actual (1.90) component families are pairwise disjoint at the cube-carrier level.
- For the canonical carrier-overlap model, C_inc=1.
- For canonical carrier overlap in d=4, tree stitching cost is c_stitch=2.
- General corrected radius formula with C_inc,c_stitch is proved.
- Canonical exact-union derivative support D_eta H_gamma^can=0 unless eta subset gamma is proved by definition of the canonical chart.

Unverified:
- exact iff definition of the prime in (1.90);
- a native Supp_* and finite source-certified C_inc;
- native c_stitch if source incompatibility is stronger than carrier overlap;
- the actual formula of cited series (7.13) [I];
- exact or bounded-change identification of native R'^(k)(X) with canonical coordinates.

Current blocker:
This run stops at the audit. The immediate certificates are exactly:

YM-RPRIME-NATIVE-COMPAT-1
and
YM-RPRIME-NATIVE-COORD-1.

No upstream Eq. (1.91) work is started.

Next action:
Recover/verify the notation behind the prime in (1.90) from the exact source lineage and locate the cited cluster series (7.13) [I] or a version/cross-reference containing it. Only after those certificates may native Balaban R' bounded C^1 be restored.

Iteration-9 verdict:
STATUS: CANONICAL R' C^1 SURVIVES; NATIVE BALABAN R' C^1 CLAIM WITHDRAWN PENDING TWO SOURCE CERTIFICATES.


## Update — Iteration 2 (YM-CROSSOVER-1 only)

STATUS:
YM-CROSSOVER-1 REDUCED TO EXPLICIT SUBLEMMAS; inclusion itself NOT proved.

Strongest new result:
The measure-only claim
R^M(K_match) subset D_KP
is insufficient for the original microscopic mass gap unless source/observable transport is controlled. The corrected crossover package is:

(A) YM-MATCH-EXTRACT-1:
extract a global, cutoff/volume-uniform K_match from the UV construction in the concrete Banach space of crossover_spec.md.

(B) YM-TUBE-1 + YM-RG-DRIFT-1:
prove finite one-step enclosures for beta and the full remainder norm. Once a uniform drift delta>0 and invariant remainder radius R exist, M_G<infinity follows and is no longer an independent heuristic assumption.

(C) YM-RG-SOURCE-1:
control source transport / conditional covariance across eliminated RG shells.

(D) Final KP landing:
Q_{alpha,mu}(z_Phi)<alpha. YM-KP-ANCHOR-1 then gives uniform cluster expansion and exponential coarse correlations.

Concrete Banach space:
B_{alpha,mu,p,rho} of globally defined gauge-invariant polymer interactions Phi_X on connected 4D block polymers, with norm
sup_B sum_{X contains B} exp(alpha|X|+mu diam(X)) ||Phi_X||_{C^p,rho}.
Full definition: crossover_spec.md.

Corrected blocker:
The hardest unresolved estimate is no longer the abstract statement "M_G finite". It is the construction of rigorous one-step Yang-Mills enclosures

F_j^-(beta,r) <= beta(R Phi) <= F_j^+(beta,r),
r(R Phi) <= G_j(beta,r)

on an invariant tube, together with source-locality estimates, starting from a rigorously extracted K_match.

Verified this iteration:
- scalar-coupling-only flow is insufficient (explicit toy counterexamples);
- q<alpha in the anchored weighted polymer norm is a sufficient KP condition;
- the finite invariant-tube induction is rigorous;
- a positive uniform beta drift plus invariant remainder tube implies cutoff-independent finite M;
- previous cutoff bridge can be strengthened to positive-time correlator convergence only (YM-CUTOFF-BRIDGE-2), so divergent equal-time renormalization constants/prefactors need not be assumed controlled.

Validated-numerics support:
scripts/verify_crossover_tube.py uses exact rational interval arithmetic to certify the finite tube induction once rigorous one-step bounds are supplied. The bundled JSON is explicitly only a toy example.


### Final refinement — Iteration 2

The one-step invariant-tube problem has now been reduced further by YM-RG-DERIV-1 and YM-RG-TAYLOR-1.

For Phi=beta W+eta with ell(eta)=0 and ||eta||<=r, define
f(beta)=ell(R(beta W)),
h(beta)=||Q R(beta W)||.
A rigorous one-step tube enclosure follows from five scalar bound families on each compact beta interval I_j:

1. center flow: f_j^- <= f(beta) <= f_j^+;
2. generated center remainder: h(beta) <= H_j;
3. relevant/irrelevant mixing: ||ell D R_{beta W} Q|| <= A_j;
4. irrelevant amplification: ||Q D R_{beta W} Q|| <= B_j;
5. curvature: sup_{Phi in K_j} ||D^2 R_Phi|| <= C_j.

It then suffices that

beta_{j+1}^- <= f_j^- - A_j R_j - (1/2)||ell|| C_j R_j^2,

beta_{j+1}^+ >= f_j^+ + A_j R_j + (1/2)||ell|| C_j R_j^2,

R_{j+1} >= H_j + B_j R_j + (1/2)||Q|| C_j R_j^2.

For the exact block-integration part of R,
D E_Phi[A]=E_{Phi,V}[A] and
D^2 E_Phi[A,B]=-Cov_{Phi,V}(A,B).
Thus the fifth bound is concretely a one-shell conditional-covariance estimate, not an abstract Frechet-regularity placeholder.

Current deepest blocker:
obtain rigorous, volume-independent Yang-Mills bounds for f,H,A,B,C on at least the first intermediate-coupling interval emerging from YM-MATCH-EXTRACT-1. This is the exact next step.


### Adversarial corrections — final

1. D_KP is NOT equivalent to a mass gap. It is only a sufficient landing chart; even exactly product measures can violate a badly chosen product-Haar activity smallness condition if local weights are large. Therefore failure of YM-CROSSOVER-1 in this chart would not imply gaplessness.

2. Polymer incompatibility must be defined using the actual collar/link support of Phi_X, not just the core block set.

3. Source control must start before the finite crossover: the UV-to-matching package must deliver quasi-local source kernels for microscopic gauge-invariant observables (YM-UV-SOURCE-MATCH-1).

4. The practical finite-drift lemma is YM-RG-DRIFT-2, with a bounded terminal beta strip and a no-overshoot estimate.

Final status of Iteration 2:
STATUS: YM-CROSSOVER-1 REDUCED TO EXPLICIT LEMMAS/INEQUALITIES — NOT PROVED.

For the measure inclusion itself, the remaining Yang-Mills content is:
- MATCH: rigorous K_match extraction in the concrete global norm;
- STEP_j: rigorous f_j^±, H_j, A_j, B_j, C_j bounds for finitely many intermediate-coupling intervals;
- LAND: final Q_{alpha,mu}<alpha estimate.

For use in the mass-gap chain, additionally:
- UV-SOURCE: microscopic-to-matching source localization;
- CROSSOVER-SOURCE: shell-by-shell source/conditional-covariance bounds.

Exact next attack:
prove or falsify the FIRST one-step certificate STEP_0 in the chosen norm, starting from the strongest matching estimates that can actually be extracted from Balaban's small-field/large-field RG results.
